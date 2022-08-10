﻿CREATE PROCEDURE [dbo].[GetAllUnitOrdersFromSpecificOrder]
	@OrderId int 
AS
BEGIN
	select OU.[StartDate], OU.EndDate, R.[Name] as 'RoomName', WP.[Number], ASer.[Name] as 'ServiceName' from dbo.[OrderUnit] as OU
	left join dbo.[WorkPlace] as WP on OU.[WorkPlaceId]=WP.[Id]
	left join dbo.[Room] as R on OU.[RoomId]=R.[Id] or WP.[RoomId]=R.[Id]
	left join dbo.[AdditionalService] as ASer on OU.AdditionalServiceId=Aser.Id
	where OU.[OrderId] = @OrderId
END
