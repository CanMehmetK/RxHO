using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RxTests.Bilgiler
{
    public class SequencesOfCoincidence
    {
        /*
            We can conceptualize events that have duration as windows. For example;
                * a server is up
                * a person is in a room
                * a button is pressed (and not yet released).
            
                The first example could be re-worded as "for this window of time, the server was up." An event from one source may have a greater value if
            it coincides with an event from another source. For example, while at a music festival, you may only be interested in tweets(events) about
            an artist while they are playing(window). In finance, you may only be interested in trades(event) for a certain instrumen while the New York
            market is open(window). In operations, you may be interested in the user sessions(window) that remained active during an upgrade of a system(windows).
            In that example, we would be querying for coinciding windows.

                Rx provides the power to query sequences of coincidence, sometimes called 'sliding windows'. We already recognize the benefit that Rx delivers
            when querying data in motion. By additionally providing the power to query sequences of coincidence, Rx exposes yet another dimension of possibilities.

                 
         
         */
    }
}
