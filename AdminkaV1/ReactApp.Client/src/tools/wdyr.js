// why-did-you-render integration for React app
import React from 'react';
import whyDidYouRender from '@welldone-software/why-did-you-render';    

if (process.env.NODE_ENV === 'development') {
  
  whyDidYouRender(React, {
    trackAllPureComponents: true,
    logOnDifferentValues: true,
  });

  console.log('WDYR enabled');
  console.log('React.memo:', React.memo);

}