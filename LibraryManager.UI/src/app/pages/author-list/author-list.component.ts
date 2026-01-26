import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthorService } from '../../services/author.service';
import { Author } from '../../models/author.model';
import { RouterLink } from '@angular/router';
import { DialogService } from '../../components/dialog-confirm/dialog.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-author-list',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './author-list.component.html',
  styleUrl: './author-list.component.scss'
})
export class AuthorListComponent implements OnInit, OnDestroy {

  authors: Author[] = [];
  subscription: Subscription | undefined

  constructor(private authorService: AuthorService, private dialogService: DialogService) { }
  
  ngOnInit(): void {
    this.subscription = this.dialogService.notify$.subscribe(result => {
      result ? this.delete() : this.cancelDelete()
    })

    this.loadAuthors();
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe()
  }

  loadAuthors(): void {
    this.authorService.getAuthors()
      .subscribe({
        next: (authors: Author[]) => this.authors = authors,
        error: (err) => this.dialogService.showError({ title: 'Fetching data failed' }, err)
      });
  }

  private toDelete: Author | null = null;
  confirmDelete(author:Author) {
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
    this.authorService.deleteAuthor(this.toDelete.id)
      .subscribe({
        next: (res) => {
          this.authors = this.authors.filter(a => a.id !== this.toDelete!.id)
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
