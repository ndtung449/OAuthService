export interface PageResult<T> {
    totalRecord: number;
    totalPage: number;
    items: Array<T>;
}
