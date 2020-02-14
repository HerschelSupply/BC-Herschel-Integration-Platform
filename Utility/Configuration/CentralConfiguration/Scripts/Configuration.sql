CREATE TABLE `Configuration` (
  `ConfigurationId` int(11) NOT NULL AUTO_INCREMENT,
  `Type` varchar(7) NOT NULL COMMENT 'Service or Process',
  `TypeId` varchar(45) NOT NULL COMMENT 'Name of Service or Process',
  `InstrumentationLevel` varchar(8) NOT NULL DEFAULT 'Normal',
  `TracingEnabled` tinyint(1) NOT NULL DEFAULT '0' COMMENT '0 will be converted to false.  All other values will equate to true.',
  PRIMARY KEY (`ConfigurationId`),
  UNIQUE KEY `ConfigurationId_UNIQUE` (`ConfigurationId`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1 COMMENT='Table containing both Service and Process level configurations'




CREATE TABLE `ConfigurationItem` (
  `ConfigurationItemId` int(11) NOT NULL AUTO_INCREMENT,
  `ConfigurationId` int(11) NOT NULL,
  `Key` varchar(50) NOT NULL,
  `Value` varchar(250) NOT NULL,
  PRIMARY KEY (`ConfigurationItemId`),
  UNIQUE KEY `ConfigurationItemId_UNIQUE` (`ConfigurationItemId`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1 COMMENT='Key value pairs repreenting service and process configurations'


CREATE DEFINER=`Martin`@`%` PROCEDURE `ConfigurationGet`(serviceId varchar(45),
											processName varchar(45))
BEGIN

SELECT `Configuration`.`Type`,
		`Configuration`.`InstrumentationLevel`,
		`Configuration`.`TracingEnabled`,
		`ConfigurationItem`.`Key`,
		`ConfigurationItem`.`Value`
	FROM `HipMgmt`.`ConfigurationItem`LEFT JOIN `HipMgmt`.`Configuration`
	ON `Configuration`.`ConfigurationId` = `ConfigurationItem`.`ConfigurationId`
	WHERE    `Configuration`.`Type` = 'Process'
	AND    `Configuration`.`TypeId` = processName
UNION
SELECT `Configuration`.`Type`,
		`Configuration`.`InstrumentationLevel`,
		`Configuration`.`TracingEnabled`,
		`ConfigurationItem`.`Key`,
		`ConfigurationItem`.`Value`
	FROM `HipMgmt`.`ConfigurationItem`LEFT JOIN `HipMgmt`.`Configuration`
	ON `Configuration`.`ConfigurationId` = `ConfigurationItem`.`ConfigurationId`
	WHERE    `Configuration`.`Type` = 'Service'
	AND    `Configuration`.`TypeId` = serviceId;
END
