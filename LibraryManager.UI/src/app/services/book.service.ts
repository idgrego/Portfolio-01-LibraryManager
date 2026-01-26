import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Book, BookCreate } from "../models/book.model";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class BookService {
    private apiURL = 'https://localhost:7072/api/book';

    constructor(private http: HttpClient) { }

    getBooks(): Observable<Book[]> {
        return this.http.get<Book[]>(this.apiURL);
    }

    getBookById(id: number): Observable<Book> {
        return this.http.get<Book>(`${this.apiURL}/${id}`);
    }

    createBook(book: BookCreate): Observable<Book> {
        return this.http.post<Book>(this.apiURL, book);
    }

    updateBook(id:number, book:Book): Observable<any> {
        return this.http.put<any>(`${this.apiURL}/${id}`, book);
    }

    deleteBook(id:number): Observable<any> {
        return this.http.delete<any>(`${this.apiURL}/${id}`);
    }
}