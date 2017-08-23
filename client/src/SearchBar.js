import React, {Component} from 'react';
import ReactCSSTransitionGroup  from 'react-transition-group';
import Autocomplete from 'react-md/lib/Autocompletes';
import CircularProgress from 'react-md/lib/Progress/CircularProgress';
import throttle from 'lodash.throttle';

import SearchService from './service/SearchService.js';

export default class SearchBar extends Component {
	constructor(props) {
		super(props);

		this.state = { hits: [], fetching: false };
		this._searchForBooks = this._searchForBooks.bind(this);
		this._onChanged = this._onChanged.bind(this);
	}

	_searchForBooks(value) {
		if (!value) {
			this.setState({ fetching: false });
			return;
		}
		this.setState({ 'fetching': true });

		var searchService = new SearchService();
		searchService.getSearchHints(value)
		.then(results => {
			const hits = results;
			this.setState({ 'hits': hits, 'fetching':false });
		})
		.catch(x => {
				console.error(x);
			});
	}

	_onChanged(value) {
		//throttle(this.props.filterBooks, 500);
		//throttle(this._searchForBooks, 250);
		this.props.filterBooks(value);
		this._searchForBooks(value);
	}

	render() {
		const { hits, fetching } = this.state;

		const progress = (
		<div className="md-cell md-cell--12" key="prorgress">
			<CircularProgress id="fetching-artist" />
		</div>
		);

		return (
			<div>
			<Autocomplete
			id="book-search"
			type="search"
			label="Type a book title"
			className="md-cell"
			placeholder="Book"
			data={hits}
			dataLabel="name"
			dataValue="id"
			filter={null}
			onChange={throttle(this._onChanged, 250)}
			clearOnAutocomplete
			onAutocomplete={this.props.filterBooks}
			/>
			{fetching ? progress : null}
			</div>
		);
	}
}