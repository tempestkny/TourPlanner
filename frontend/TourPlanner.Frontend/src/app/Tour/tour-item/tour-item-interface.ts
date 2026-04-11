export interface TourItemInterface {
    id : string;
    userId:string;
    title: string;
    tourDescription?: string;
    from: string;
    to: string;
    transportType?: string;
}
