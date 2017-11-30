import React, { Component } from 'react';

import './BookView.css';
import TextField from 'react-md/lib/TextFields';
import Paper from 'react-md/lib/Papers';
import Button from 'react-md/lib/Buttons/Button';

class BookView extends Component {

	constructor(props){
		super(props);

		this.linkMap = {
			0: "mobi",
			1: "epub"
		  };
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

	buildFilesList(files){
		return files
		.map(file => (<li> + file.relativeFilePath + </li>));
	}

	buildLink(book){
		return <a href={"content/" + book.relativeFilePath}>{this.linkMap[book.fileType]}</a>;
	}

    render(){
        return(
            <Paper className="book-paper">
				<div className="book">
					<figure>
						<img src={"content/" + this.props.book.coverImagePath} title="Cover image"/>
					</figure>
					<div className="bookdescription">
						<p>Title: {this.props.book.title}</p>
						<p>Author(s): {this.joinAuthors(this.props.book.authors)}</p>
						<ul>
							{this.props.book.files.map(x => <li>{this.buildLink(x)}</li>)}
						</ul>
					</div>
				</div>
				<Button raised label="Back" onClick={this.props.goBack}/>
                <Button raised label="Send" onClick={this.props.send}/>
            </Paper>
        );
    }
}

export default BookView;