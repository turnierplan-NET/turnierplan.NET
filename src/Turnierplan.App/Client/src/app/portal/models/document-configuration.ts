import { MatchPlanDocumentConfiguration } from '../../api-temporary';
import { ReceiptsDocumentConfiguration } from '../../api-temporary';

type EmptyDocumentConfiguration = { [key: string]: never };

export type DocumentConfiguration = EmptyDocumentConfiguration | MatchPlanDocumentConfiguration | ReceiptsDocumentConfiguration;
