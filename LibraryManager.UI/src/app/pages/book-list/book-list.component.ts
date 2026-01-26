import { Component, OnDestroy, OnInit } from '@angular/core';
import { Book } from '../../models/book.model';
import { Subscription } from 'rxjs';
import { DialogService } from '../../components/dialog-confirm/dialog.service';
import { BookService } from '../../services/book.service';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';


@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [RouterLink, DatePipe],
  templateUrl: './book-list.component.html',
  styleUrl: './book-list.component.scss'
})
export class BookListComponent implements OnInit, OnDestroy {

  books: Book[] = [];
  subscription: Subscription | undefined

  constructor(
    private bookService: BookService, 
    private dialogService: DialogService) { }
  
  ngOnInit(): void {
    this.subscription = this.dialogService.notify$.subscribe(result => {
      result ? this.delete() : this.cancelDelete()
    })

    this.loadBooks();
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe()
  }

  loadBooks(): void {
    this.bookService.getBooks()
      .subscribe({
        next: (books: Book[]) => this.books = books,
        error: (err) => this.dialogService.showError({ title: 'Fetching data failed' }, err)
      });
  }

  private toDelete: Book | null = null;
  confirmDelete(author:Book) {
    this.toDelete = author;
    this.dialogService.show({
      title: 'Confirm deletion',
      message: 'Are you sure you want to delete this author?',
      confirmText: 'Delete',
      cancelText: 'Cancel',
      isNotification: false
    })
  }

  delete() {
    if (!this.toDelete) return;
    this.bookService.deleteBook(this.toDelete.id)
      .subscribe({
        next: (res) => {
          this.books = this.books.filter(a => a.id !== this.toDelete!.id)
          this.cancelDelete()
        },
        error: (err) => {
          this.cancelDelete()
          this.dialogService.showError({ title: 'Deleting author failed' }, err)
        }
      })
  }

  cancelDelete() {
    this.toDelete = null;
  }
}
