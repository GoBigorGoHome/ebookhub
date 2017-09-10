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
				<div className="book">
					<img src={"content/" + this.props.book.coverImagePath} title="Cover image"/>
					<div className="bookdescription">
						<p className="bookprop">Title: {this.props.book.title}</p>
						<p className="bookprop">Author(s): {this.joinAuthors(this.props.book.authors)}</p>
					</div>
				</div>
				<Button raised label="Back" onClick={this.props.goBack}/>
                <Button raised label="Send" onClick={this.props.send}/>
            </Paper>
        );
    }
}

export default BookView;