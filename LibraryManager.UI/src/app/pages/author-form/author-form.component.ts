import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { Author } from '../../models/author.model';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthorService } from '../../services/author.service';
import { DialogService } from '../../components/dialog-confirm/dialog.service';

@Component({
  selector: 'app-author-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './author-form.component.html',
  styleUrl: './author-form.component.scss'
})
export class AuthorFormComponent implements OnInit {

  @Output() authorCreated = new EventEmitter();

  toEdit: Author | null = null;
  authorForm: FormGroup

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private authorService: AuthorService,
    private dialogService: DialogService
  ) {
    this.authorForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]]
    })
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.fetchAuthor(id)
  }
  
  fetchAuthor(id: string | null) {
    if (id) {
      this.authorService.getAuthorById(+id).subscribe({
        next: (author) => {
          this.toEdit = author;
          this.authorForm.patchValue({ name: author.name });
        },
        error: (err) => this.dialogService.showError({ title: 'Fetching data failed' }, err)
      });
    } else {
      this.toEdit = null;
    }
  }

  onCancel() {
    this.router.navigate(['/authors']); // Volta sem salvar
  }

  onReset() {
    if (this.toEdit) {
      this.authorForm.reset({ name: this.toEdit.name });
    } else {
      this.authorForm.reset();
    }
  }

  onSubmit() {
    if (this.authorForm.invalid) {
      return;
    }

    if (this.toEdit) {
      const toSave = {
        id: this.toEdit.id,
        name: this.authorForm.value.name
      }

      this.authorService.updateAuthor(this.toEdit.id, toSave)
        .subscribe({
          next: (res) => this.onCancel(),
          error: (err) => this.dialogService.showError({ title: 'Updating author failed' }, err)
        })

    } else
      this.authorService.createAuthor(this.authorForm.value)
        .subscribe({
          next: (res) => this.onCancel(),
          error: (err) => this.dialogService.showError({ title: 'Creating author failed' }, err)
        })
  }

}
