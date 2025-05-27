import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  standalone: false,
  name: 'abstractTeamSelector'
})
export class AbstractTeamSelectorPipe implements PipeTransform {
  private static transformGroupRankingSelectorDE(position: number, groupId: string): string {
    return `${position}. Gruppe ${groupId}`;
  }

  private static transformNthRankedSelectorDE(ordinalNumber: number, groupPosition: number): string {
    const ordinalsBest = ['Bester', 'Zweitbester', 'Drittbester', 'Viertbester'];
    const ordinalsIndex = ['erster', 'zweiter', 'dritter', 'vierter', 'fünfter', 'sechster', 'siebter', 'achter'];

    const ordinalBest = ordinalNumber < ordinalsBest.length ? ordinalsBest[ordinalNumber] : `${ordinalNumber + 1}.-bester`;
    const ordinalIndex = groupPosition < ordinalsIndex.length ? ordinalsIndex[groupPosition] : `-${groupPosition + 1}ter`;

    return `${ordinalBest} Gruppen${ordinalIndex}`;
  }

  public transform(abstractTeamSelector: string, groupAlphabeticalIds: string[], lang: 'de'): string {
    if (!abstractTeamSelector) {
      return '';
    }

    if (lang !== 'de') {
      return abstractTeamSelector;
    }

    const data = /^(\d+)([B.])(\d+)$/.exec(abstractTeamSelector);

    if (data) {
      const first = +data[1];
      const separator = data[2];
      const second = +data[3];

      switch (separator) {
        case '.':
          return AbstractTeamSelectorPipe.transformGroupRankingSelectorDE(first, groupAlphabeticalIds[second]);
        case 'B':
          return AbstractTeamSelectorPipe.transformNthRankedSelectorDE(first, second - 1);
      }
    }

    return abstractTeamSelector;
  }
}
