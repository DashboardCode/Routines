module.exports = {
  // Define the file paths to the bundles you want to monitor
  files: [
    './wwwroot/dist/main.js',  
    './wwwroot/dist/main.css', 
    './wwwroot/dist/vendor.css',
    './wwwroot/dist/vendor.js',
    './wwwroot/dist/runtime.js'
  ],

  // Set the size limits for your bundles (e.g., size limit in bytes for gzip)
  gzip: true,  // Enable gzip size checks
  limits: {
    'main.js': {
      maxSize: "200 kB",  // Maximum size in bytes for the main bundle
      maxGzip: "200 kB",   // Maximum gzip size for the main bundle
    },
    'main.css': {
      maxSize: "200 kB",  // Maximum size in bytes for the main bundle
      maxGzip: "200 kB",   // Maximum gzip size for the main bundle
    },
    'vendor.css': {
      maxSize: "200 kB",  // Maximum size in bytes for the main bundle
      maxGzip: "200 kB",   // Maximum gzip size for the main bundle
    },
    'vendor.js': {
      maxSize: "950 kB",  // Maximum size in bytes for the vendor bundle
      maxGzip: "950 kB",   // Maximum gzip size for the vendor bundle
    },
    'runtime.js': {
      maxSize: "10 kB",  // Maximum size in bytes for the vendor bundle
      maxGzip: "10 kB",   // Maximum gzip size for the vendor bundle
    }
  },

  // Optionally, you can configure notifications and failure thresholds
  threshold: 10000, // Set a threshold for failing if the bundle size exceeds it
}