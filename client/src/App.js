import React, { Component } from 'react';
import logo from './logo.svg';
import './App.css';
import SearchableList from './SearchableList';
import Settings from './Settings';
import Toolbar from 'react-md/lib/Toolbars/Toolbar';
import Button from 'react-md/lib/Buttons/Button';
import 'react-md/dist/react-md.amber-blue.min.css';

class App extends Component {
	constructor(props)
	{
		 super(props);

		 this.state = {
			 'settingsVisible' : false
		 }

		 this._toggleSettings = this._toggleSettings.bind(this);
	}

	_toggleSettings(){
		let old = this.state.settingsVisible;
		this.setState({'settingsVisible' : !old});
	}

 	render() {

		const actions = [
			<Button icon onClick={this._toggleSettings}>settings</Button>
		];

		if(this.state.settingsVisible === true)
		{
			return(
				<div>
					<Toolbar colored title="Library" actions={actions}/>
					<Settings onBack={this._toggleSettings}/>
				</div>
			);
		}

		return(
			<div>
				<Toolbar colored title="Library" actions={actions}/>
				<SearchableList/>
			</div>
		);
  	}
}

export default App;
