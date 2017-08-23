import 'whatwg-fetch';

class BookService {

    getAllBooks() {

        return fetch(process.env.REACT_APP_API_HOST + '/api/books', {
            'method': 'GET'
        })
        .then(this.checkStatus)
        .then(this.parseJSON);
    }

    sendBook(bookID) {
        console.log(JSON.stringify(bookID));
        return fetch(process.env.REACT_APP_API_HOST + '/api/books/send?bookID=' + bookID, {
            'method': 'POST'
        })
        .then(this.checkStatus);
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

export default BookService;

