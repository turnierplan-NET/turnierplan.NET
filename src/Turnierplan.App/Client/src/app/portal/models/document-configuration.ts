import { MatchPlanDocumentConfiguration } from '../../api/models/match-plan-document-configuration';
import { ReceiptsDocumentConfiguration } from '../../api/models/receipts-document-configuration';

type EmptyDocumentConfiguration = { [key: string]: never };

export type DocumentConfiguration = EmptyDocumentConfiguration | MatchPlanDocumentConfiguration | ReceiptsDocumentConfiguration;
