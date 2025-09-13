const { defineConfig } = require('cypress');

module.exports = defineConfig({
  e2e: {
    setupNodeEvents(on, config) {
      config.baseUrl = 'http://localhost:45001';

      config.viewportWidth = 1600;
      config.viewportHeight = 900;

      return config;
    }
  }
});
