
-- Alle Karten, aller Listen, aller Boards

Select Board.Id as "Board Id", Board.BusinessId as "Board BusinessId", Board.Title as "Board Titel", Board.Accessibility as "Board Sichtbarkeit", Board.ArchivedAt as "Archiviert", List.BusinessId as "Listen Id", List.Title as "Listen Titel", Card.BusinessId as "Karten Id", Card.Title as "Karten Titel"
from tblCard as Card 
left join tblList as List on List.Id = Card.List_Id
left join tblBoard as Board on Board.Id = List.Board_Id

Select * from tblUser