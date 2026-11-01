const PROXY_CONFIG = [
  {
    context: ['/favicon.ico', '/favicon.svg', '/api', '/auth', '/images'],
    target: 'http://127.0.0.1:3350',
    secure: false,
    headers: {
      Connection: 'Keep-Alive'
    }
  }
];

module.exports = PROXY_CONFIG;
