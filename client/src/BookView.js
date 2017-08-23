import React, { Component } from 'react';

import TextField from 'react-md/lib/TextFields';
import Paper from 'react-md/lib/Papers';
import Button from 'react-md/lib/Buttons/Button';

class BookView extends Component {

	constructor(props){
		super(props);
	}

	joinAuthors(authors){
		return authors.map(v => (
			v.fullName
		)).join('; ')
	}

	joinTags(tags){
		return tags.map(v => (
			v.name
		)).join('; ')
	}

    render(){
        return(
            <Paper className="book-paper">
                <TextField id="Title" label="Title" disabled={true} value={this.props.book.title}/>
                <TextField id="Author" label="Author(s)" disabled={true} value={this.joinAuthors(this.props.book.authors)}/>
                <Button raised label="Back" onClick={this.props.goBack}/>
                <Button raised label="Send" onClick={this.props.send}/>
            </Paper>
        );
    }
}

export default BookView;