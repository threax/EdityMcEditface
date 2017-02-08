import * as EdityClient from 'edity.editorcore.EdityClient';
import * as PageStart from 'edity.editorcore.EditorPageStart';
import * as event from 'hr.eventdispatcher';
import { ExternalPromise } from 'hr.externalpromise';

export class CompilerServiceEventArgs {
    service: CompilerService;

    constructor(service: CompilerService) {
        this.service = service;
    }
}

export class CompilerServiceErrorEventArgs extends CompilerServiceEventArgs {
    error: any;

    constructor(service: CompilerService, error: any) {
        super(service);
        this.error = error;
    }
}

export class CompilerServiceStatusEventArgs extends CompilerServiceEventArgs {
    status: CompilerStatus;

    constructor(service: CompilerService, status: CompilerStatus) {
        super(service);
        this.status = status;
    }
}

export interface CompilerStatus {
    message: string;
}

export interface CompilerPhase {
    execute: (arg: CompilerServiceEventArgs) => Promise<any>;
}

class PrimaryCompilePhase implements CompilerPhase {
    private compileClient: EdityClient.CompileClient;

    constructor(pageStart: PageStart.EditorPageStart) {
        this.compileClient = new EdityClient.CompileClient(pageStart.BaseUrl, pageStart.Fetcher);
    }

    execute(arg: CompilerServiceEventArgs) {
        arg.service.setStatus({ message: "Publishing Website" })
        return this.compileClient.compile(null)
            .then(r => {
                arg.service.setStatus({ message: "Website compiled in " + r.elapsedSeconds + " seconds." });
                return r;
            });
    }
}

export class CompilerService {
    private startedEvent = new event.ActionEventDispatcher<CompilerServiceEventArgs>();
    private statusUpdatedEvent = new event.ActionEventDispatcher<CompilerServiceStatusEventArgs>();
    private successEvent = new event.ActionEventDispatcher<CompilerServiceEventArgs>();
    private failedEvent = new event.ActionEventDispatcher<CompilerServiceErrorEventArgs>();

    private status: CompilerStatus;

    private phases: CompilerPhase[] = [];

    constructor(pageStart: PageStart.EditorPageStart) {
        this.phases.push(new PrimaryCompilePhase(pageStart));
    }

    public compile(): Promise<any> {
        this.startedEvent.fire(new CompilerServiceEventArgs(this));

        var compilePromise = new ExternalPromise<any>();

        var currentPhase: CompilerPhase;
        var currentPromise: Promise<any>;
        var i = 0;
        var runPhase = () => {
            if (i < this.phases.length) {
                currentPhase = this.phases[i++];
                currentPhase.execute(new CompilerServiceEventArgs(this))
                    .then(r => {
                        runPhase();
                    })
                    .catch(err => {
                        compilePromise.reject(err);
                    });
            }
            else {
                compilePromise.resolve();
            }
        };
        runPhase();

        return compilePromise.Promise.then(r => {
            this.successEvent.fire(new CompilerServiceEventArgs(this));
            return r;
        })
        .catch(err => {
            this.failedEvent.fire(new CompilerServiceErrorEventArgs(this, err));
            return err;
        });
    }

    public setStatus(status: CompilerStatus) {
        this.status = status;
        this.statusUpdatedEvent.fire(new CompilerServiceStatusEventArgs(this, status));
    }

    public addCompilationPhase(phase: CompilerPhase) {
        this.phases.push(phase);
    }

    public get onStarted() {
        return this.startedEvent.modifier;
    }

    public get onStatusUpdated() {
        return this.statusUpdatedEvent.modifier;
    }

    public get onSuccess() {
        return this.successEvent.modifier;
    }

    public get onFailed() {
        return this.failedEvent.modifier;
    }
}