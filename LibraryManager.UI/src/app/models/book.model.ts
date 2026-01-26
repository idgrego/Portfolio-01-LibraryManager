export interface Book {
    id: number
    title: string
    isbn: string
    publishedDate: Date
    authorId: number
    authorName?: string
}

export interface BookCreate {
    title: string
    isbn: string
    authorId: number,
    publishedDate: string
}

export interface BookUpdate {
    id: number
    title: string
    isbn: string
    authorId: number,
    publishedDate: string
}