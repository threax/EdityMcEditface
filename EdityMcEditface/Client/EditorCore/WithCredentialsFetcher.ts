import { Fetcher } from 'hr.Fetcher';

/**
 * A fetcher that adds credentials.
 * @param {type} next - The next fetcher in the chain.
 * @returns
 */
export class WithCredentialsFetcher implements Fetcher {
    private next: Fetcher;

    constructor(next: Fetcher) {
        this.next = next;
    }

    fetch(url: RequestInfo, init?: RequestInit): Promise<Response> {
        init.credentials = "include";
        return this.next.fetch(url, init);
    }
}