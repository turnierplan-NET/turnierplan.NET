console.log('Generating TypeScript code for IAM actions/roles...');

const sourcePath = '../Security/Actions.cs';
console.log(`Loading .cs source file from: '${sourcePath}'`);

const fs = require('node:fs');
const content = fs.readFileSync(sourcePath, 'utf8');

console.log('Applying regex to extract data');
const regex = /^\s+public static readonly Action (\w+) = new\((Role\.\w+(?:, Role\.\w+)*)\);$/gm;
const matches = [...content.matchAll(regex)];
console.log(`Found ${matches.length} matches:`);

let generatedActionDefinitions = '';

for (let i = 0; i < matches.length; i++) {
  const match = matches[i];
  const actionName = match[1];
  const rolesList = match[2];
  const processedRoles = rolesList
    .split(',')
    .flatMap((x) => [...x.matchAll(/Role\.(\w+)/g)])
    .map((x) => x[1]);

  console.log(` -> Found action '${actionName}' with roles: ${processedRoles.join(', ')}`);

  generatedActionDefinitions += `  public static readonly ${actionName} = new Action([`;
  for (let j = 0; j < processedRoles.length; j++) {
    generatedActionDefinitions += 'Role.';
    generatedActionDefinitions += processedRoles[j];
    if (j < processedRoles.length - 1) {
      generatedActionDefinitions += ', ';
    }
  }
  generatedActionDefinitions += ']);';
  if (i < matches.length - 1) {
    generatedActionDefinitions += '\n';
  }
}

const generatedContent = `/* tslint:disable */
/* eslint-disable */
/* This file is auto-generated based on the 'Actions.cs' C# source file */

import { Role } from '../api/models/role';

export class Action {
  constructor(private readonly requiredRoles: Role[]) {}

  public isAllowed(availableRoles: Role[]): boolean {
    return availableRoles.some((role) => this.requiredRoles.includes(role));
  }
}

export class Actions {
${generatedActionDefinitions}
}

`;

const destinationFolder = 'src/app/generated';
const destinationPath = `${destinationFolder}/actions.ts`;
console.log(`Writing generated code to: '${destinationPath}'`);
if (!fs.existsSync(destinationPath)) fs.mkdirSync(destinationFolder);
fs.writeFileSync(destinationPath, generatedContent);

console.log('Code generation finished.');
