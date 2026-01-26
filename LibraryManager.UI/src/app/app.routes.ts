import { Routes } from '@angular/router';
import { AuthorListComponent } from './pages/author-list/author-list.component';
import { AuthorFormComponent } from './pages/author-form/author-form.component';
import { BookListComponent } from './pages/book-list/book-list.component';
import { BookFormComponent } from './pages/book-form/book-form.component';
import { PageNotFoundComponent } from './pages/page-not-found/page-not-found.component';

export const routes: Routes = [
    { path: '', redirectTo: 'authors', pathMatch: 'full' },
    { path: 'authors', component: AuthorListComponent },
    { path: 'authors/new', component: AuthorFormComponent },
    { path: 'authors/edit/:id', component: AuthorFormComponent },
    { path: 'books', component: BookListComponent },
    { path: 'books/new', component: BookFormComponent },
    { path: 'books/edit/:id', component: BookFormComponent },
    { path: '**', component: PageNotFoundComponent }
];
