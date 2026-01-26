import { Component, OnInit } from '@angular/core';
import { AuthorService } from '../../services/author.service';
import { BookService } from '../../services/book.service';
import { DialogErrorData, DialogService } from '../../components/dialog-confirm/dialog.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Book } from '../../models/book.model';
import { Author } from '../../models/author.model';

@Component({
  selector: 'app-book-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './book-form.component.html',
  styleUrl: './book-form.component.scss'
})
export class BookFormComponent implements OnInit {

  toEdit: Book | null = null;
  bookForm: FormGroup
  authors: Author[] = []


  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private authorService: AuthorService, 
    private bookService: BookService, 
    private dialogService: DialogService) {
      this.bookForm = this.formBuilder.group({
        title: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
        isbn: ['', [Validators.required, Validators.minLength(13), Validators.maxLength(13)]],
        authorId: ['', [Validators.required]],
        publishedDate: ['', [Validators.required]]
      })
    }

  ngOnInit(): void {
    // 1. Carrega os autores para o Select
    this.authorService.getAuthors().subscribe({
      next: data => this.authors = data,
      error: (err) => this.dialogService.showError({ title: 'Fetching author list failed' }, err)
    });

    // 2. Verifica se é edição
    const id = this.route.snapshot.paramMap.get('id');
    this.fetchBook(id)
  }
  
  fetchBook(id: string | null) {
    if (id) {
      this.bookService.getBookById(+id).subscribe({
        next: (book) => {
          this.toEdit = book
          this.bookForm.patchValue({
            title: book.title,
            isbn: book.isbn,
            authorId: book.authorId,
            publishedDate: new Date(book.publishedDate).toISOString().split('T')[0]
          });
        },
        error: (err) => this.dialogService.showError({ title: 'Fetching data failed' }, err)
      });
    } else {
      this.toEdit = null;
    }
  }

  onCancel() {
    this.router.navigate(['/books']); // Volta sem salvar
  }

  onReset() {
    if (this.toEdit) {
      this.bookForm.reset({
            title: this.toEdit.title,
            isbn: this.toEdit.isbn,
            authorId: this.toEdit.authorId,
            publishedDate: new Date(this.toEdit.publishedDate).toISOString().split('T')[0]
          });
    } else {
      this.bookForm.reset();
    }
  }

  onSubmit() {
    if (this.bookForm.invalid) {
      return;
    }

    const date = this.bookForm.value.publishedDate + 'T12:00:00';

    if (this.toEdit) {
      const toSave = {
        id: this.toEdit.id,
        title: this.bookForm.value.title,
        isbn: this.bookForm.value.isbn,
        authorId: this.bookForm.value.authorId,
        publishedDate: new Date(date).toISOString()
      }

      this.bookService.updateBook(this.toEdit.id, toSave)
        .subscribe({
          next: (res) => this.onCancel(),
          error: (err) => this.handleError(err, 'Updating book failed')
        })

    } else {
      const toSave = {
        title: this.bookForm.value.title,
        isbn: this.bookForm.value.isbn,
        authorId: this.bookForm.value.authorId,
        publishedDate: new Date(date).toISOString()
      }
      
      this.bookService.createBook(toSave)
        .subscribe({
          next: (res) => this.onCancel(),
          error: (err) => this.handleError(err, 'Creating book failed')
        })
    }
  }

  private handleError(err: any, title: string) {
    const data: DialogErrorData = { title }
            
    if (err.error.detail.includes('IX_Book_ISBN_Unique'))
      data.message = 'ISBN already exists'
    else if (err.error.detail.includes('IX_Book_Title_AuthorId_Unique')) 
      data.message = 'This author already has a book with this title'

    this.dialogService.showError(data, err)
  }

}
