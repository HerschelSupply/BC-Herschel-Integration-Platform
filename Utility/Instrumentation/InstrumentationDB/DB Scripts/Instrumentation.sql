CREATE TABLE `Interchange` (
  `InterchangeId` varchar(40) NOT NULL,
  `InterchangeTimestamp` datetime NOT NULL,
  `ProcessName` varchar(45) NOT NULL,
  `EntryPoint` varchar(45) NOT NULL,
  `ImplementationType` varchar(10) NOT NULL COMMENT 'Values should be Messaging or SSIS',
  `CreateTimestamp` datetime DEFAULT CURRENT_TIMESTAMP,
  `BatchId` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`InterchangeId`),
  UNIQUE KEY `InterchangeId_UNIQUE` (`InterchangeId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='Core table of the instrumentation tables'


CREATE TABLE `Service` (
  `ServiceInstanceId` varchar(40) NOT NULL,
  `IntechangeId` varchar(40) NOT NULL,
  `ServiceId` varchar(45) NOT NULL,
  `Version` decimal(4,2) DEFAULT NULL,
  `ServiceOperationId` varchar(45) DEFAULT NULL,
  `ServiceOperationInstanceId` varchar(40) DEFAULT NULL,
  `MachineName` varchar(40) DEFAULT NULL COMMENT 'Name of server on which the service instance is running.',
  `CreateTimestamp` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`ServiceInstanceId`),
  UNIQUE KEY `ServiceInstanceId_UNIQUE` (`ServiceInstanceId`),
  KEY `InterchangeId_idx` (`IntechangeId`),
  CONSTRAINT `InterchangeId` FOREIGN KEY (`IntechangeId`) REFERENCES `Interchange` (`InterchangeId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='The service instances involved in the interchange'


CREATE TABLE `ServicePostOperation` (
  `ServicePostOperationInstanceId` varchar(40) NOT NULL,
  `ServiceInstanceId` varchar(40) NOT NULL,
  `ServicePostOperationId` varchar(45) NOT NULL,
  `DestinationUrl` varchar(264) DEFAULT NULL COMMENT 'If a destiation URL has been dynamically returned it should be documented here.',
  `RetryCount` int(11) DEFAULT '0' COMMENT 'If the service call to the detination fails each following attempt will increment the retry count.',
  `CreateTimestamp` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`ServicePostOperationInstanceId`),
  KEY `ServiceToServicePostOperation_idx` (`ServiceInstanceId`),
  CONSTRAINT `ServiceToServicePostOperation` FOREIGN KEY (`ServiceInstanceId`) REFERENCES `Service` (`ServiceInstanceId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='Used to track service post operations from a service instance'


CREATE TABLE `Message` (
  `MessageId` varchar(40) NOT NULL,
  `ServiceInstanceId` varchar(40) DEFAULT NULL COMMENT 'The message needs to be related to either a service for received messages or a service post operation for sent messages.',
  `ServicePostOperationInstanceId` varchar(40) DEFAULT NULL COMMENT 'The message needs to be related to either a service for received messages or a service post operation for sent messages.',
  `MessageType` varchar(150) NOT NULL COMMENT 'Fully qualified message class.  IE corp.integration.eConnect',
  `Version` decimal(4,2) DEFAULT NULL,
  `ParentMessageId` varchar(40) DEFAULT NULL,
  `Topic` varchar(45) DEFAULT NULL,
  `FilterKeyValuePairs` varchar(150) DEFAULT NULL,
  `ProcessKeyValuePairs` varchar(150) DEFAULT NULL,
  `ProcessEnd` tinyint(1) DEFAULT '0',
  `FullMessage` blob COMMENT 'Used to store full message for verbose tracking',
  `CreateTimestamp` datetime DEFAULT CURRENT_TIMESTAMP,
  `MessageSplitIndex` smallint(5) DEFAULT NULL,
  `DocumentId` varchar(150) DEFAULT '0' COMMENT 'This field contains the document ID from the payload message.  It has been promoted to the messages envelope and captured in the DB to support troubleshooting of a specific message.  If no document ID is available the default value of 0 will be used.',
  PRIMARY KEY (`MessageId`),
  UNIQUE KEY `MessageId_UNIQUE` (`MessageId`),
  KEY `ServiceToMessage_idx` (`ServiceInstanceId`),
  CONSTRAINT `ServiceToMessage` FOREIGN KEY (`ServiceInstanceId`) REFERENCES `Service` (`ServiceInstanceId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='Used to capture message meta data during an interchange'


CREATE TABLE `Exception` (
  `ExceptionId` int(11) NOT NULL AUTO_INCREMENT,
  `InterchangeId` varchar(40) NOT NULL,
  `MessageId` varchar(40) DEFAULT NULL,
  `ParentMessageId` varchar(40) DEFAULT NULL,
  `FullMessage` blob,
  `ExceptionMessage` blob,
  `StackTrace` blob,
  `MachineName` varchar(40) DEFAULT NULL,
  `CreateTimestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`ExceptionId`),
  UNIQUE KEY `ExceptionId_UNIQUE` (`ExceptionId`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=latin1


CREATE TABLE `ActivationLog` (
  `ServiceActivationId` int(11) NOT NULL AUTO_INCREMENT,
  `ActivationGuid` varchar(40) NOT NULL,
  `Server` varchar(45) DEFAULT NULL,
  `IsBlocked` tinyint(1) NOT NULL DEFAULT '0',
  `Service` varchar(45) DEFAULT NULL,
  `StartTimeStamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `EndTimeStamp` datetime DEFAULT NULL COMMENT 'This field will be null if an exception occurs during processing unless multiple interchanges will be associated with the activation.',
  PRIMARY KEY (`ServiceActivationId`),
  UNIQUE KEY `ServiceActivationId_UNIQUE` (`ServiceActivationId`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1 COMMENT='Tracks the activation of services even if it doesn''t result in messages being processed.'



CREATE TABLE `ActivationInterchangeJoin` (
  `ActivationInterchangeId` int(11) NOT NULL AUTO_INCREMENT,
  `ActivationGuid` varchar(40) NOT NULL,
  `InterchangeGuid` varchar(40) NOT NULL,
  PRIMARY KEY (`ActivationInterchangeId`),
  UNIQUE KEY `ActivationInterchangeId_UNIQUE` (`ActivationInterchangeId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1 COMMENT='Allows the sevice activations to be accosiated with the interchanges that the activation generated.'


CREATE TABLE `ServerStatus` (
  `ServerStatusId` int(11) NOT NULL AUTO_INCREMENT,
  `ServerName` varchar(45) NOT NULL,
  `ServerIp` varchar(15) NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1' COMMENT 'When a server is deactivated the flag should be switched to 0 and a deactivation timestamp entered.',
  `ActivationTimestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `DeactivationTimestamp` datetime DEFAULT NULL,
  `DeactivatedBy` varchar(45) DEFAULT NULL COMMENT 'Server name that deactivated record.  If self deactivated, value should be''Self''',
  PRIMARY KEY (`ServerStatusId`),
  UNIQUE KEY `ServerStatusId_UNIQUE` (`ServerStatusId`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=latin1 COMMENT='Tracks the activation and deactivation of servers in the HIP services cluster'




CREATE DEFINER=`Martin`@`%` PROCEDURE `InterchangeExists`(id varchar(40))
BEGIN
SELECT 
	CASE 
		WHEN count(InterchangeId) > 0 
        THEN 'True'
        ELSE 'False'
	END AS Exist
FROM Interchange
where InterchangeId = id;
END

CREATE DEFINER=`Martin`@`%` PROCEDURE `InterchangeInsert`(interchangeId varchar(40), 
														interchangeTimestamp datetime, 
                                                        processName varchar(45), 
                                                        entryPoint varchar(45), 
                                                        implementationType varchar(10),
                                                        batchId varchar(40))
BEGIN

INSERT 
	INTO `HipMgmt`.`Interchange`
		(`InterchangeId`,
		`InterchangeTimestamp`,
		`ProcessName`,
		`EntryPoint`,
		`ImplementationType`,
        `BatchId`)
	VALUES
		(interchangeId,
		interchangeTimestamp,
		processName,
		entryPoint,
		implementationType,
        batchId);
END


CREATE DEFINER=`Martin`@`%` PROCEDURE `MessageExists`(msgId varchar(40))
BEGIN
SELECT 
	CASE 
		WHEN count(MessageId) > 0 
        THEN 'True'
        ELSE 'False'
	END AS Exist
FROM Message
WHERE MessageId = msgId;
END

CREATE DEFINER=`Martin`@`%` PROCEDURE `MessageInsert`(messageId varchar(40), 
									serviceInstanceId varchar(40),
                                    servicePostOperationInstanceId varchar(40),
                                    messageType varchar(150),
                                    version decimal(4,2),
                                    parentMessageId varchar(40),
                                    topic varchar(45),
                                    filterKeyValuePairs varchar(150),
                                    processKeyValuePairs varchar(150),
                                    processEnd tinyint(1),
                                    messageSplitIndex smallint(5),
                                    documentId varchar(150),
                                    fullMessage blob)
BEGIN

INSERT 
	INTO `HipMgmt`.`Message`
		(`MessageId`,
		`ServiceInstanceId`,
		`ServicePostOperationInstanceId`,
		`MessageType`,
		`Version`,
		`ParentMessageId`,
		`Topic`,
		`FilterKeyValuePairs`,
        `ProcessKeyValuePairs`,
        `ProcessEnd`,
        `MessageSplitIndex`,
        `DocumentId`,
		`FullMessage`)
	VALUES
		(messageId,
		serviceInstanceId,
		servicePostOperationInstanceId,
		messageType,
		version,
		parentMessageId,
		topic,
		filterKeyValuePairs,
        processKeyValuePairs,
        processEnd,
        messageSplitIndex,
        documentId,
		fullMessage);
END


CREATE DEFINER=`Martin`@`%` PROCEDURE `ServiceExists`(instanceId varchar(40))
BEGIN
SELECT 
	CASE 
		WHEN count(ServiceInstanceId) > 0 
        THEN 'True'
        ELSE 'False'
	END AS Exist
FROM Service
where ServiceInstanceId = instanceId;
END

CREATE DEFINER=`Martin`@`%` PROCEDURE `ServiceInsert`(serviceInstanceId varchar(40), 
									intechangeId varchar(40), 
									serviceId varchar(45), 
									version decimal(4,2), 
									serviceOperationId varchar(45), 
                                    serviceOperationInstanceId varchar(40),
                                    machineName varchar(40))
BEGIN

INSERT 
	INTO `HipMgmt`.`Service`
		(`ServiceInstanceId`,
		`IntechangeId`,
		`ServiceId`,
		`Version`,
		`ServiceOperationId`,
		`ServiceOperationInstanceId`,
        `MachineName`)
	VALUES
		(serviceInstanceId,
		intechangeId,
		serviceId,
		version,
		serviceOperationId,
		serviceOperationInstanceId,
        machineName);
END


CREATE DEFINER=`Martin`@`%` PROCEDURE `ServicePostOpExists`(instanceId varchar(40))
BEGIN
SELECT 
	CASE 
		WHEN count(ServicePostOperationInstanceId) > 0 
        THEN 'True'
        ELSE 'False'
	END AS Exist
FROM ServicePostOperation
where ServicePostOperationInstanceId = instanceId;
END

CREATE DEFINER=`Martin`@`%` PROCEDURE `ServicePostOpInsert`(servicePostOperationInstanceId varchar(40), 
										serviceInstanceId varchar(40),
                                        servicePostOperationId varchar(45),
                                        destinationUrl varchar(264),
                                        retryCount int)
BEGIN

INSERT 
	INTO `HipMgmt`.`ServicePostOperation`
		(`ServicePostOperationInstanceId`,
		`ServiceInstanceId`,
		`ServicePostOperationId`,
		`DestinationUrl`,
		`RetryCount`)
	VALUES
		(servicePostOperationInstanceId,
		serviceInstanceId,
		servicePostOperationId,
		destinationUrl,
		retryCount);
END


CREATE DEFINER=`Martin`@`%` PROCEDURE `ExceptionInsert`(interchangeId varchar(40),
									messageId varchar(40),
 									parentMessageId varchar(40),
                                    msg blob,
                                    exceptionMsg blob,
                                    stackTrace blob,
                                    machineName varchar(40))
BEGIN
INSERT 
	INTO `HipMgmt`.`Exception`
		(`InterchangeId`,
		`FullMessage`,
		`ExceptionMessage`,
		`StackTrace`,
		`MessageId`,
        `ParentMessageId`,
        `MachineName`)
	VALUES
		(interchangeId,
		msg,
		exceptionMsg,
		stackTrace,
		messageId,
        parentMessageId,
        machineName);
END



CREATE DEFINER=`Martin`@`%` PROCEDURE `ActivationLogInsert`(activationGuid varchar(40),
										serverName varchar(45),
										service varchar(45),
										isBlocked tinyint(1))
BEGIN

INSERT INTO `HipMgmt`.`ActivationLog`
(`ActivationGuid`,
`Server`,
`Service`,
`IsBlocked`)
VALUES
(activationGuid,
serverName,
service,
isBlocked);

END


CREATE DEFINER=`Martin`@`%` PROCEDURE `ActivationLogEndUpdate`(activationGuid varchar(40))
BEGIN

UPDATE `HipMgmt`.`ActivationLog`
SET
`ActivationLog`.`EndTimeStamp` = CURRENT_TIMESTAMP
WHERE (`ActivationLog`.`ActivationGuid` = activationGuid);

END


CREATE DEFINER=`Martin`@`%` PROCEDURE `ActivationInterchangeJoinInsert`(activationGuid varchar(40),
													interchangeGuid varchar(40))
BEGIN

INSERT INTO `HipMgmt`.`ActivationInterchangeJoin`
(`ActivationGuid`,
`InterchangeGuid`)
VALUES
(activationGuid,
interchangeGuid);

END



CREATE DEFINER=`Martin`@`%` PROCEDURE `ServerStatusGetActiveServers`()
BEGIN

SELECT `ServerStatus`.`ServerIp`
FROM `HipMgmt`.`ServerStatus`
WHERE `ServerStatus`.`IsActive` = 1;

END


CREATE DEFINER=`Martin`@`%` PROCEDURE `ServerStatusGetServerCount`()
BEGIN

SELECT COUNT(`ServerStatus`.`ServerIp`) AS ServerCount
FROM `HipMgmt`.`ServerStatus`
WHERE `ServerStatus`.`IsActive` = 1;

END



CREATE DEFINER=`Martin`@`%` PROCEDURE `ServerStatusLogActivation`(serverName varchar(45), serverIp varchar(15))
BEGIN

INSERT INTO `HipMgmt`.`ServerStatus`
(`ServerName`,
`ServerIp`)
VALUES
(serverName,
ServerIp);

END


CREATE DEFINER=`Martin`@`%` PROCEDURE `ServerStatusLogDeactivation`(deactivatedBy varchar(45), 
																	targetServerIp varchar(15))
BEGIN

SET @recordId = (SELECT `ServerStatus`.`ServerStatusId`
		FROM `HipMgmt`.`ServerStatus`
		WHERE `ServerStatus`.`ServerIp` = targetServerIp
		AND `ServerStatus`.`IsActive` = 1
		ORDER BY `ServerStatus`.`ServerStatusId` Desc
		LIMIT 1);


UPDATE `HipMgmt`.`ServerStatus`
SET
`IsActive` = 0,
`DeactivationTimestamp` = CURRENT_TIMESTAMP,
`DeactivatedBy` = deactivatedBy
WHERE `ServerStatusId` = @recordId;

END





