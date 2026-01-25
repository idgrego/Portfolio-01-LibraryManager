import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthorService } from '../../services/author.services';
import { Author } from '../../models/author.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-author-list',
  standalone: true,
  imports: [],
  templateUrl: './author-list.component.html',
  styleUrl: './author-list.component.scss'
})
export class AuthorListComponent implements OnInit, OnDestroy {

  authors: Author[] = [];
  subscription: Subscription | undefined

  constructor(private authorService: AuthorService) { }

  ngOnInit(): void {
    // 1. Escuta o canal de refresh
    this.subscription = this.authorService.refreshNeeded$.subscribe(() => {
      this.loadAuthors(); // Recarrega a lista sempre que o sinal for emitido
    });

    // 2. Carga inicial
    this.loadAuthors();
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  loadAuthors(): void {
    console.log('busca listagem de autores')
    this.authorService.getAuthors()
      .subscribe({
        next: (authors: Author[]) => this.authors = authors,
        error: (error) => {
          console.error('Error fetching authors:', error);
        }
      });
  }

  delete(id: number) {
    console.log('excluíndo autor', id)
    this.authorService.deleteAuthor(id)
      .subscribe({
        next: (res) => {
          console.log('Autor deletado com sucesso', res);
          this.authorService.notifyChange()
        },
        error: (err) => {
          console.error('Erro ao deletar um autor:', err);
        }
      })
  }

  edit(author: Author) {
    this.authorService.selectAuthorForEdit(author)
  }
}
