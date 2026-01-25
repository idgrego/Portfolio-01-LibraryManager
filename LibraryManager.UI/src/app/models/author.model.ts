export interface Author {
    id: number
    name: string;
    books?: any[];
}

export interface AuthorCreate {
    name: string;
}