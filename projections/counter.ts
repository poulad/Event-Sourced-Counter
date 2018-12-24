/// <reference path="event-store.d.ts" />

fromCategory('counter')
    .when({
        $init: () => { return []; },
        $any: (state, event) => {
            state.push(event);
        },
    })
