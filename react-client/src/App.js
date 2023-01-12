import logo from './logo.svg';
import './App.css';
import useWebSocket from 'react-use-websocket';

const WS_URL = "ws://localhost:8080/ws/depthquotes";
var symbol = '';
var bids;
var asks;

function App() {
  console.log(process.env);
  useWebSocket(process.env.REACT_APP_WS_URL, {
    onOpen: () => {
      console.log('WebSocket connection established.');
    },
    onMessage: (event) => {
      const json = JSON.parse(event.data);

      symbol = json.Channel.replace('public.depth.', '').toUpperCase();
      
      bids = json.Bids.map((item) => {
        return (
          <div className="divTableRow">
            <div className="divTableCellBid">{item.Price}</div>
            <div className="divTableCell">{item.Quantity}</div>
          </div>
        )
      });

      var sortedAsks = json.Asks.sort((a, b) => b.Price - a.Price);
      asks = sortedAsks.map((item) => {
        return (
          <div className="divTableRow">
            <div className="divTableCellAsk">{item.Price}</div>
            <div className="divTableCell">{item.Quantity}</div>
          </div>
        )
      });

      try {
        
      } catch (err) {
        console.log(err);
      }
    },
  });

  


  return (
    <div className="App">
      <div>
        <b>{symbol}</b>
      </div>
      <div className="divTable">
      <div className="divTableBody">
          <div className="divTableRow">
            <div className="divTableHead">Price</div>
            <div className="divTableHead">Quantity</div>
          </div>
          {asks}
          {bids}
        </div>
      </div>
    </div>
  );
}

export default App;
