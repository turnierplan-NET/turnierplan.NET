import { TranslateService } from '@ngx-translate/core';

import { DocumentType } from '../../api';

export const getDocumentName = (type: DocumentType, translateService: TranslateService): string => {
  return translateService.instant(`Portal.ViewTournament.Documents.Types.${type}`) as string;
};
