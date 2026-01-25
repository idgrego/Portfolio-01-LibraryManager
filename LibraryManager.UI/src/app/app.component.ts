import { Component } from '@angular/core';
import { AuthorListComponent } from './components/author-list/author-list.component';
import { AuthorFormComponent } from './components/author-form/author-form.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [AuthorListComponent, AuthorFormComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'LibraryManager-UI';
}
