import { ListingModel } from "./listing.model";

export interface ListingDetailModel extends ListingModel {
    description: string;
    photoUrls: string[];
    wants: string[];
    priceAmount: number;
    priceCurrency: string;
    category: string;
    condition: string;
    latitude: number;
    longitude: number;
}
