#@@@@@@@@@@@@ Set Date to clean up before
SET @timestamp = '2018-02-16 6:00';

#@@@@@@@@@@@@ Clean up Exception Records
#Some exceptions will not be based on Interchanges
#START TRANSACTION;
DELETE FROM HipMgmt.Exception
WHERE CreateTimestamp < @timestamp;
#ROLLBACK;

#@@@@@@@@@@@@ Clean up Activation Log
#SET @timestamp = '2018-01-01';
#START TRANSACTION;
DELETE FROM HipMgmt.ActivationLog
#WHERE StartTimeStamp < @timestamp;
WHERE ActivationGuid IN (	SELECT DISTINCT ActivationGuid
								FROM HipMgmt.ActivationInterchangeJoin AIJ
								LEFT JOIN HipMgmt.Interchange I ON I.InterchangeId = AIJ.InterchangeGuid
								WHERE I.CreateTimestamp < @timestamp);
#ROLLBACK;

#@@@@@@@@@@@@ Clean up Activation Interchange Join Records
#SET @timestamp = '2018-01-01';
#START TRANSACTION;
DELETE FROM HipMgmt.ActivationInterchangeJoin
WHERE InterchangeGuid IN (	SELECT InterchangeId
							FROM HipMgmt.Interchange 
							WHERE CreateTimestamp < @timestamp);
#ROLLBACK;

#@@@@@@@@@@@@ Clean up Message Records
#START TRANSACTION;
#SET @timestamp = '2018-01-01';
DELETE FROM HipMgmt.Message
WHERE ServiceInstanceId IN (	SELECT ServiceInstanceId 
								FROM HipMgmt.Service S 
								LEFT JOIN HipMgmt.Interchange I on I.InterchangeId = S.InterchangeId
								WHERE I.CreateTimestamp < @timestamp);
#SELECT MessageId FROM HipMgmt.Message WHERE CreateTimestamp < '2018-01-01';
#ROLLBACK;

#START TRANSACTION;
#DELETE FROM HipMgmt.Message
#WHERE CreateTimestamp < '2017-12-31';
#ROLLBACK;


#@@@@@@@@@@@@ Clean up Service Post Operations Records
#START TRANSACTION;
#SET @timestamp = '2018-01-01';
DELETE FROM ServicePostOperation
WHERE ServiceInstanceId IN (SELECT ServiceInstanceId 
											FROM  HipMgmt.Service S 
											LEFT JOIN HipMgmt.Interchange I on I.InterchangeId = S.InterchangeId
											WHERE I.CreateTimestamp < @timestamp);
#SELECT ServicePostOperationInstanceId FROM HipMgmt.ServicePostOperation WHERE CreateTimestamp < '2018-01-01';
#ROLLBACK;

#@@@@@@@@@@@@ Clean up Service Records
#START TRANSACTION;
#SET @timestamp = '2018-01-01';
DELETE FROM Service
WHERE InterchangeId IN (	SELECT InterchangeId
								FROM HipMgmt.Interchange I 
								WHERE I.CreateTimestamp < @timestamp);
#SELECT ServiceInstanceId FROM HipMgmt.Service S WHERE CreateTimestamp < '2018-01-01';
#ROLLBACK;

#@@@@@@@@@@@@ Clean up Interchange Records
#START TRANSACTION;
#SET @timestamp = '2018-01-01';
DELETE FROM Interchange
WHERE CreateTimestamp < @timestamp;
#ROLLBACK;




