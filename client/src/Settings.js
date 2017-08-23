import React, {Component} from 'react';
import TextField from 'react-md/lib/TextFields';
import Button from 'react-md/lib/Buttons';
import CircularProgress from 'react-md/lib/Progress/CircularProgress'

import SettingsService from './service/SettingsService';

export default class Settings extends Component {

    constructor(props){
        super(props);

        this.state = {
            'isLoading': true,
            'user': {
                'id': '',
                'kindleMail': '',
                'name': ''
            }
        };

        this._updateSettings = this._updateSettings.bind(this);
        this._handleChange = this._handleChange.bind(this);        
    }

    _updateSettings(){
        let settingsService = new SettingsService();
		settingsService.updateSettings(this.state.user)
		.catch(x => {
            console.error(x);
        });

        this.props.onBack()
    }

    _handleChange(v, event){
        const target = event.target;
        const name = target.name;

        let user = this.state.user;
        user[name] = v;
        
        this.setState({
            'user': user
        });
    }

    componentDidMount(){
		let settingsService = new SettingsService();
		settingsService.getSettings()
		.then(user => {
			this.setState({
				'isLoading': false,
				'user': user
			});
		})
		.catch(x => {
            this.setState({
				'isLoading': false
			});
            console.error(x);
        });
	}

    render() {
        if(this.state.isLoading === true)
        {
            return(
				<CircularProgress key="progress" id="1" />
			);
        }

        return(
            <div>
                <TextField name="kindleMail" id="kindleMail" placeholder="user.name@kindle.com" 
                type="email" className="md-cell md-cell--bottom" onChange={this._handleChange}
                value={this.state.user.kindleMail}/>
                <Button raised label="Save" onClick={this._updateSettings}></Button>
            </div>
        );
    }
    
}

