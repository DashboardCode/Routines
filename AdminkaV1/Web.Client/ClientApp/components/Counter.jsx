import * as React from 'react';
import { connect } from 'react-redux';
import * as CounterStore from '../store/Counter';
class Counter extends React.Component {
    render() {
        return <div>
            <h1>Counter</h1>

            <p>This is a simple example of a React component.</p>

            <p>Current count: <strong>{this.props.count}</strong></p>

            <button onClick={() => { this.props.increment(); }}>Increment</button>
        </div>;
    }
}
// Wire up the React component to the Redux store
export default connect((state) => state.counter, // Selects which state properties are merged into the component's props
CounterStore.actionCreators // Selects which action creators are merged into the component's props
)(Counter);
//# sourceMappingURL=Counter.jsx.map