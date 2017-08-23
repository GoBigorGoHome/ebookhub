import 'whatwg-fetch';

class SearchService {

    getSearchHints(searchTerm) {
        var url = process.env.REACT_APP_API_HOST + '/api/search?searchTerm=' + searchTerm;
        return fetch(url, {
            'method': 'GET'
        })
        .then(this.checkStatus)
        .then(this.parseJSON);
    }

    searchBooks(searchTerm) {
        var url = process.env.REACT_APP_API_HOST + '/api/search/books?searchTerm=' + searchTerm;
        return fetch(url, {
            'method': 'GET'
        })
        .then(this.checkStatus)
        .then(this.parseJSON);
    }

    checkStatus(response) {
        if (response.status >= 200 && response.status < 300) {
            return response;
        }
        const error = new Error(response.statusText);
        error.response = response;
        throw error;
    }

    parseJSON(response) {
        return response.json();
    }
}

export default SearchService;

