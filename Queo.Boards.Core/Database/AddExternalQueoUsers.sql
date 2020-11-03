insert into tblUser 
		([BusinessId] , [UserName]  ,[Firstname]    , [Lastname]      ,[Email]                       , [Phone]     , [Company]           , [IsEnabled] , [PasswordHash]                                                         ,[UserCategory]    )     
values 
        (NEWID()      , 'mklose'    , 'Michael'     , 'Klose'         , 'm.klose@queo-group.com'     , ''          , 'queo GmbH'         , 1           , 'AOE1azFemyZHNE2qQRZveBdwB3n0wSBqxyKjNUgsOobA/pKu/fHPocgvfPlUEcs35Q==' , 'Local'),
		(NEWID()      , 'tmoritz'   , 'Thomas'      , 'Moritz'        , 't.moritz@queo-group.com'    , ''          , 'queo GmbH'         , 1           , 'ACsnMcKnAO8T6nc1PtUYRmTEmfO2uIeVuwqa6yjzBS4fwieAXKIVP9knDtImUF8qKw==' , 'Local'),
		(NEWID()      , 'culbrich'  , 'Christian'   , 'Ulbrich'       , 'c.ulbrich@queo-group.com'   , ''          , 'queo GmbH'         , 1           , 'AGZW5R75He4cyJBmOTigRhqWAzH+//HKTC6ZCJBisIUpUYu6CMJrOSGR/lkGdTUSPA==' , 'Local'),
		(NEWID()      , 'tjaekel'   , 'Tobias'      , 'Jäkel'         , 't.jaekel@queo-group.com'    , ''          , 'queo GmbH'         , 1           , 'AG+XU29VobYfPMvU4bYaEbeWrKR4G6mDwFMbLQAFyw7oLpufb5cYdq32E23WbhwXkg==' , 'Local'),
		(NEWID()      , 'bboruttau' , 'Bernd'       , 'Boruttau'      , 'b.boruttau@queo-group.com'  , ''          , 'queo GmbH'         , 1           , 'AD3qacloUe6X4B1DursR3hf+i1yIuKyhkg/ITsg4Y6AUrLDd83iRfz5fjrTVyY56BA==' , 'Local'),
		(NEWID()      , 'uoehmichen', 'Uwe'         , 'Oehmichen'     , 'u.oehmichen@queo-group.com' , ''          , 'queo GmbH'         , 1           , 'AA6Dl0+bP4sGO31ke0eM5tPVU2L/B3xmt/4XX9JYQJv7ndVCAmkK/K+4cAPwxN4TKA==' , 'Local'),
		(NEWID()      , 'mwinkler'  , 'Matthes'     , 'Winkler'       , 'm.winkler@queo-group.com'   , ''          , 'queo GmbH'         , 1           , 'AK9zgspFlBBjgsJkOdrgOPy+++Pag04KJIKrZ9mPx4Jc0Eshic+2sgkrN6umMnV60A==' , 'Local'),
		(NEWID()      , 'cosmar'    , 'Dirk'        , 'Cosmar'        , 'd.cosmar@queo-group.com'    , ''          , 'queo GmbH'         , 1           , 'AJwdn2RwYbycAwmZtdqi8+V9div1wCwJsQD8zP1cjxG2WVkjvlIzjmBQB+V2QG1t6g==' , 'Local'),
		(NEWID()      , 'rschmidt'  , 'Ricardo'     , 'Schmidt'       , 'r.schmidt@queo-group.com'   , ''          , 'queo GmbH'         , 1           , 'ANw1oaq7RfDoDPPBwuBpe+ewipZXpH3olvSg75pObt882/PYjCLIeDBC0dDtHAR0FQ==' , 'Local'),
		(NEWID()      , 'aok-nds'   , 'AOK'         , 'Niedersachsen' , 'rene.ballueer@nds.aok.de'   , ''          , 'AOK Niedersachsen' , 1           , 'ADTATzL7NmLmaX/kysUH8skr3dcebpc3P2ylypVN4U07ODLWSFhOOH7v1B1dsJlHpQ==' , 'Local');

