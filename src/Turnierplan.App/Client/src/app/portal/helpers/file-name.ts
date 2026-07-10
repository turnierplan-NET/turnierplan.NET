export const makeSafeFileName = (input: string): string => {
  return input.replaceAll(/[^.A-Za-z0-9Ä-Öä-öß _-]/g, '_');
};
