import 'whatwg-fetch';

class SettingsService {

    getSettings(){
        return fetch(process.env.REACT_APP_API_HOST + '/api/user', {
            'method': 'GET'
        })
        .then(this.checkStatus)
        .then(this.parseJSON);
    }

    updateSettings(settings) {
        return fetch(process.env.REACT_APP_API_HOST + '/api/user', {
            'method': 'POST',
            'headers': {
                'Content-Type': 'application/json'
            },
            'body': JSON.stringify(settings)
        })
        .then(this.checkStatus);
    }

    checkStatus(response) {
        if (response.status >= 200 && response.status < 300) {
            return response;
        }
        const error = new Error(response.statusText);
        throw error;
    }

    parseJSON(response) {
        return response.json();
    }
}

export default SettingsService;

