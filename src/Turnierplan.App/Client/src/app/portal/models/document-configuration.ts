import { MatchPlanDocumentConfiguration, ReceiptsDocumentConfiguration } from '../../api';

type EmptyDocumentConfiguration = { [key: string]: never };

export type DocumentConfiguration = EmptyDocumentConfiguration | MatchPlanDocumentConfiguration | ReceiptsDocumentConfiguration;
