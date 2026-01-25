import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";
import { Author, AuthorCreate } from "../models/author.model";

@Injectable({
    providedIn: 'root'
})
export class AuthorService {
    private apiURL = 'https://localhost:7072/api/author';


    constructor(private http: HttpClient) { }

    // =====================================

    readonly refreshNeeded$ = new Subject<void>();

    notifyChange() {
        setTimeout(() => {
            this.refreshNeeded$.next();
        }, 100)
    }

    // =====================================

    readonly authorSelected$ = new Subject<Author>();

    selectAuthorForEdit(author: Author) {
        this.authorSelected$.next(author);
    }

    // =====================================

    getAuthors(): Observable<Author[]> {
        return this.http.get<Author[]>(this.apiURL);
    }

    createAuthor(author: AuthorCreate): Observable<Author> {
        return this.http.post<Author>(this.apiURL, author)
    }

    updateAuthor(id: number, author: Author): Observable<any> {
        return this.http.put(`${this.apiURL}/${id}`, author)
    }

    deleteAuthor(id: number): Observable<any> {
        return this.http.delete(`${this.apiURL}/${id}`)
    }
}