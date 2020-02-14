#*********** Count of messages per Interchange ********************************
SELECT tI.InterchangeId, tI.CreateTimestamp, tI.ProcessName, Count(tM.MessageId)
FROM HipMgmt.Interchange tI
LEFT JOIN HipMgmt.Service tS on tS.InterchangeId = tI.InterchangeId
LEFT JOIN HipMgmt.Message tM on tM.ServiceInstanceId = tS.ServiceInstanceId
WHERE tI.InterchangeId in 
	(SELECT InterchangeId FROM HipMgmt.Interchange
	WHERE CreateTimestamp > '2018-02-23 20:50')
AND ProcessEnd = 1
GROUP BY tI.InterchangeId, tI.CreateTimestamp, tI.ProcessName
ORDER BY tI.CreateTimestamp Desc;


#*********** Count of messages sent to an Off-Ramp ********************************
SELECT tI.InterchangeId, tI.CreateTimestamp, tI.ProcessName, Count(tM.MessageId)
FROM HipMgmt.Interchange tI
LEFT JOIN HipMgmt.Service tS on tS.InterchangeId = tI.InterchangeId
LEFT JOIN HipMgmt.ServicePostOperation tSPO  on tSPO.ServiceInstanceId = tS.ServiceInstanceId
LEFT JOIN HipMgmt.Message tM on tM.ServicePostOperationInstanceId = tSPO.ServicePostOperationInstanceId
WHERE tI.InterchangeId in 
	(SELECT InterchangeId FROM HipMgmt.Interchange
	WHERE CreateTimestamp > '2018-02-11 00:00')
AND tSPO.ServicePostOperationId = 'MessageSubscriptionPush'
GROUP BY tI.InterchangeId, tI.CreateTimestamp, tI.ProcessName
ORDER BY tI.CreateTimestamp Desc;


#*********** Count of messages placed on Queue ********************************
SELECT tI.InterchangeId, tI.CreateTimestamp, tI.ProcessName, Count(tM.MessageId)
FROM HipMgmt.Interchange tI
LEFT JOIN HipMgmt.Service tS on tS.InterchangeId = tI.InterchangeId
LEFT JOIN HipMgmt.ServicePostOperation tSPO  on tSPO.ServiceInstanceId = tS.ServiceInstanceId
LEFT JOIN HipMgmt.Message tM on tM.ServicePostOperationInstanceId = tSPO.ServicePostOperationInstanceId
WHERE tI.InterchangeId in 
	(SELECT InterchangeId FROM HipMgmt.Interchange
	WHERE CreateTimestamp > '2018-02-15 21:40')
AND tSPO.ServicePostOperationId = 'QueueNriShipping'
GROUP BY tI.InterchangeId, tI.CreateTimestamp, tI.ProcessName
ORDER BY tI.CreateTimestamp Desc;

#*********** Exceptions ********************************

SELECT * FROM HipMgmt.Exception
WHERE CreateTimestamp > '2018-02-23 20:50'
#AND CHAR_LENGTH(InterchangeId) > 1
order by CreateTimestamp Desc;


#*********** Delivered Perf Test Documents ********************************

SELECT * FROM HipMgmt.PerfTest
WHERE DateTimeStamp > '2018-02-23 20:50'
;

#*********** Activations ********************************

SELECT * FROM HipMgmt.ActivationLog
WHERE HipMgmt.ActivationLog.StartTimeStamp > '2018-02-23 18:15'
ORDER BY ActivationLog.StartTimeStamp Desc;


#****** Rate of processing (PerfTest) **********************************

SET @Min = (SELECT MIN(Message.CreateTimestamp)
    FROM HipMgmt.Message
    WHERE MessageType = 'eConnect'
    AND CreateTimestamp > '2018-02-23 22:12');

SET @Max = (SELECT MAX(Message.CreateTimestamp)
    FROM HipMgmt.Message
    WHERE MessageType = 'eConnect'
    AND CreateTimestamp < '2018-02-23 22:15');


SET @count = (SELECT COUNT(Message.ServiceInstanceId)
		FROM HipMgmt.Message
		WHERE CreateTimestamp > @Min
		AND CreateTimestamp < @Max
		AND MessageType = 'eConnect');
 
 SELECT @Min AS Min, @Max AS Max, TIMESTAMPDIFF(SECOND, @min, @Max) AS Duration,  @count as Count, (@Count/TIMESTAMPDIFF(SECOND, @min, @Max)) AS AvgRate;


#*********** Logged Message steps from failed message ID (Walks down the heirarchy to the start msg) ********************************

SET @msgId = '60fef389-0868-427a-9ef3-2ef357cbef01';
#SET @msgId = '03885da9-ce36-482a-b110-9da78d8c7096';
#SET @msgId = 'f616f394-e202-4712-a432-7252b9c85545';
#SET @msgId = 'd50adae7-20fd-4549-8111-ccad7b80f019';
#SET @msgId = 'e6250323-33fb-4109-bbaa-d1ab9d441224';
#SET @msgId = '4a90d7b2-1ba4-4385-b139-1fa87a124ec4';
#SET @msgId = '6e507c2c-8800-4dc4-a3b7-17fb427d1869';
SET @msgId = 'cff3feb1-b67c-40d1-b745-827b2c8c062e';

SET @ID1 = (SELECT ParentMessageId FROM HipMgmt.Message WHERE MessageId = @msgId);
SET @ID2 = (SELECT ParentMessageId FROM HipMgmt.Message WHERE MessageId = @ID1);
SET @ID3 = (SELECT ParentMessageId FROM HipMgmt.Message WHERE MessageId = @ID2);
SET @ID4 = (SELECT ParentMessageId FROM HipMgmt.Message WHERE MessageId = @ID3);
SET @ID5 = (SELECT ParentMessageId FROM HipMgmt.Message WHERE MessageId = @ID4);
SELECT DISTINCT tI.InterchangeId, tI.ProcessName, tS.ServiceId, tS.ServiceOperationId, null as 'ServicePostOperationId', null as 'Exception'
FROM HipMgmt.Interchange tI
LEFT JOIN HipMgmt.Service tS on tS.IntechangeId = tI.InterchangeId
LEFT JOIN HipMgmt.Message tM on tM.ServiceInstanceId = tS.ServiceInstanceId
WHERE tM.MessageId in (@msgId, @ID1, @ID2, @ID3, @ID4, @ID5)
UNION
SELECT tI.InterchangeId, tI.ProcessName, tS.ServiceId, tS.ServiceOperationId, tSPO.ServicePostOperationId, null as 'Exception'
FROM HipMgmt.Interchange tI
LEFT JOIN HipMgmt.Service tS on tS.IntechangeId = tI.InterchangeId
LEFT JOIN HipMgmt.ServicePostOperation tSPO  on tSPO.ServiceInstanceId = tS.ServiceInstanceId
LEFT JOIN HipMgmt.Message tM on tM.ServicePostOperationInstanceId = tSPO.ServicePostOperationInstanceId
WHERE tM.MessageId in (@msgId, @ID1, @ID2, @ID3, @ID4, @ID5);


#*********** Walk up the hierachy to the last message in the process ********************************

SET @msgId = '81e07da3-de17-4e06-8d9f-d86535cb709d';

SET @ID1 = (SELECT MessageId FROM HipMgmt.Message WHERE ParentMessageId = @msgId);
SET @ID2 = (SELECT MessageId FROM HipMgmt.Message WHERE ParentMessageId = @ID1);
SET @ID3 = (SELECT MessageId FROM HipMgmt.Message WHERE ParentMessageId = @ID2);
SET @ID4 = (SELECT MessageId FROM HipMgmt.Message WHERE ParentMessageId = @ID3);
SET @ID5 = (SELECT MessageId FROM HipMgmt.Message WHERE ParentMessageId = @ID4);
SELECT @msgId, @ID1, @ID2, @ID3, @ID4, @ID5;


#****** Rate of processing NRI Testing **********************************

SET @Min = (SELECT MIN(Message.CreateTimestamp)
    FROM HipMgmt.Message
    WHERE MessageType = 'Corp.Intergration.EPShippingConfirmation'
    AND CreateTimestamp > '2018-02-16 06:30');

SET @Max = (SELECT MAX(Message.CreateTimestamp)
    FROM HipMgmt.Message
    WHERE MessageType = 'Corp.Intergration.EPShippingConfirmation'
    AND CreateTimestamp < '2018-02-16 06:40');


SET @count = (SELECT COUNT(Message.ServiceInstanceId)
		FROM HipMgmt.Message
		WHERE CreateTimestamp > @Min
		AND CreateTimestamp < @Max
		AND MessageType = 'Corp.Intergration.EPShippingConfirmation');
 
 SELECT @Min AS Min, @Max AS Max, TIMESTAMPDIFF(SECOND, @min, @Max) AS Duration,  @count as Count, (@Count/TIMESTAMPDIFF(SECOND, @min, @Max)) AS AvgRate;


#***********  ********************************

#***********  ********************************


