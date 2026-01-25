import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthorService } from '../../services/author.services';
import { Subscription } from 'rxjs';
import { Author } from '../../models/author.model';

@Component({
  selector: 'app-author-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './author-form.component.html',
  styleUrl: './author-form.component.scss'
})
export class AuthorFormComponent implements OnInit {

  @Output() authorCreated = new EventEmitter();

  toEdit: Author | null = null
  authorForm: FormGroup
  subscription: Subscription | undefined

  constructor(
    private formBuilder: FormBuilder,
    private authorService: AuthorService
  ) {
    this.authorForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]]
    })
  }

  ngOnInit(): void {
    this.subscription = this.authorService.authorSelected$.subscribe((a: Author) => {
      this.toEdit = a;
      this.authorForm.patchValue({ name: a.name }); // Preenche o campo
    });
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
          next: (res) => {
            console.log('Autor atualizado com sucesso', res);
            this.reset()
          },
          error: (err) => {
            console.error('Erro ao atualizado um autor:', err);
          }
        })

    } else
      this.authorService.createAuthor(this.authorForm.value)
        .subscribe({
          next: (res) => {
            console.log('Autor cadastrado com sucesso', res);
            this.reset()
          },
          error: (err) => {
            console.error('Erro ao cadastrar um autor:', err);
          }
        })
  }

  reset() {
    this.toEdit = null
    this.authorForm.reset();
    this.authorService.notifyChange()
  }
}
