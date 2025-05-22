const compileVersionNumber = (): string => {
  const now = new Date();

  const start = new Date(now.getFullYear(), 0, 0);
  const diff = now.getTime() - start.getTime();
  const oneDay = 1000 * 60 * 60 * 24;
  const dayOfYear = Math.floor(diff / oneDay);
  const secondsOfDay = Math.floor((now.getTime() % (1000 * 60 * 60 * 24)) / 1000);

  const nowString = `${now.getFullYear()}${dayOfYear}.${secondsOfDay}`;
  return `dev-${nowString}`;
};

export const environment = {
  production: false,
  defaultTitle: 'turnierplan.NET',
  version: compileVersionNumber(),
  originOverwrite: 'http://localhost:45000'
};
