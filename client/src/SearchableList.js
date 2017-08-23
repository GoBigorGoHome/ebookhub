import React, { Component } from 'react';
import CircularProgress from 'react-md/lib/Progress/CircularProgress'
import BookList from './BookList.js';
import SearchBar from './SearchBar.js';
import BookService from './service/BookService';
import SearchService from './service/SearchService.js';
import BookView from './BookView.js';

class SearchableList extends Component {
    constructor(props)
    {
        super(props);

		this.state = {
			'isLoading': true,
			'isShowingDetails': false,
            'books': [],
			'book': null
		}
		
		this._filterBooks = this._filterBooks.bind(this);
		this._showBookDetails = this._showBookDetails.bind(this);
		this._goBack = this._goBack.bind(this);
		this._send = this._send.bind(this);
    }

	_filterBooks(value)
	{
		let searchService = new SearchService();
		searchService.searchBooks(value)
		.then(books => {
			this.setState({
				'isLoading': false,
				'books': books
			});
		})
		.catch(x => {
            console.error(x);
        });
	}

	componentDidMount(){
		let bookService = new BookService();
		bookService.getAllBooks()
		.then(books => {
			this.setState({
				'isLoading': false,
				'books': books
			});
		})
		.catch(x => {
            console.error(x);
        });
	}

	_showBookDetails(book)
	{
		this.setState({
			'isShowingDetails': true,
			'book': book
		});
	}

	_goBack()
	{
		this.setState({
			'isShowingDetails': false
		});
	}

	_send()
	{
		console.log("sending book...");
		let bookService = new BookService();
		bookService.sendBook(this.state.book.id)
		.catch(x => {
            console.error(x);
        });
	}

    render(){

		if(this.state.isLoading)
		{
			return(
				<CircularProgress key="progress" id="1" />
			);
		}

		if(this.state.isShowingDetails)
		{
			return(
				<BookView book={this.state.book} goBack={this._goBack} send={this._send}/>
			);
		}

        return(
            <div>
                <SearchBar filterBooks={this._filterBooks}/>
                <BookList books={this.state.books} showBookDetails={this._showBookDetails}/>
            </div>
        );
    }
}

export default SearchableList;