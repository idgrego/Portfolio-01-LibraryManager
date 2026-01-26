export interface Book {
    id: number
    title: string
    isbn: string
    authorId: number
    authorName?: string
}

export interface BookCreate {
    title: string
    isbn: string
    authorId: number
}

export interface BookUpdate {
    id: number
    title: string
    isbn: string
    authorId: number
}