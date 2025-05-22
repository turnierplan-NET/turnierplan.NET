const PROXY_CONFIG = [
  {
    context: ['/api', '/identity', '/images'],
    target: 'http://127.0.0.1:45000',
    secure: false,
    headers: {
      Connection: 'Keep-Alive'
    }
  }
];

module.exports = PROXY_CONFIG;
