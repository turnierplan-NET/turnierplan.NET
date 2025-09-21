type EmptyDocumentConfiguration = { [key: string]: never };

export type DocumentConfiguration = EmptyDocumentConfiguration | MatchPlanDocumentConfiguration | ReceiptsDocumentConfiguration;
