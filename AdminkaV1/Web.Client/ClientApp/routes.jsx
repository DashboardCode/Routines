import * as React from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import Home from './components/Home';
import FetchData from './components/FetchData';
import Counter from './components/Counter';
export default <Route component={Layout}>
    <Route path='/' components={{ body: Home }}/>
    <Route path='/counter' components={{ body: Counter }}/>
    <Route path='/fetchdata' components={{ body: FetchData }}>
        <Route path='(:startDateIndex)'/> 
    </Route>
</Route>;
// Enable Hot Module Replacement (HMR)
if (module.hot) {
    module.hot.accept();
}
//# sourceMappingURL=routes.jsx.map