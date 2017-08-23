import React, { Component } from 'react';
import DataTable from 'react-md/lib/DataTables/DataTable';
import TableHeader from 'react-md/lib/DataTables/TableHeader';
import TableBody from 'react-md/lib/DataTables/TableBody';
import TableRow from 'react-md/lib/DataTables/TableRow';
import TableColumn from 'react-md/lib/DataTables/TableColumn';

class BookList extends Component {

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

		let books = [];

		if(this.props.books)
		{
			books = this.props.books.map((v, i) => (
				<TableRow key={i} onClick={this.props.showBookDetails.bind(null, v)}>
					<TableColumn>{v.title}</TableColumn>
					<TableColumn>{this.joinAuthors(v.authors)}</TableColumn>
					<TableColumn>{this.joinTags(v.tags)}</TableColumn>
				</TableRow>
			));
		}

        return(
            <DataTable plain>
				<TableHeader>
					<TableRow>
						<TableColumn>Title</TableColumn>
						<TableColumn>Author(s)</TableColumn>
						<TableColumn>Tags</TableColumn>
					</TableRow>
				</TableHeader>
					<TableBody>
						{books}
					</TableBody>
			</DataTable>
        );
    }
}

export default BookList;