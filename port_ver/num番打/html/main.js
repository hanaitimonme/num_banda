$(function() {
  $.getJSON("well_known.json" , function(data) {

  //要素の格納
  var random_idx = [];

  //0~19の連番を挿入
  for(var idx=0; idx<data.length; idx++){
    random_idx[idx] = idx;
  }

  var r_cnt = data.length;
  var rand;

  var q_cnt = 0;  //問題のカウンタ

  var mis_cnt = 0;   //不正解のカウンタ

  var question_cnt = 0;
  var question_array = [];

  var question_hash = {};

  rand = Math.floor( Math.random() * r_cnt);  //ランダム値の初期化
  var answer = data[random_idx[rand]].port;  //ランダムで取り出したポート番号が入る
  var question = data[random_idx[rand]].protocol; //ランダムで取り出したプロトコル名が入る






  // var audio = new Audio(../img/enter.mp3);

   console.log(data[random_idx[rand]].port); //チェック用

  //入力ボックス内からデータを取り出し、比較する処理
  document.getElementById("portnum").onkeypress = function(e){
    if(e,e.keyCode == 13){
      q_cnt++;
      if(q_cnt < 5){
        document.getElementById("counter").textContent=q_cnt+1;
        if(answer != document.getElementById("portnum").value){
          // console.log("不正解")
          mis_cnt++;

          question_hash = {
            port: answer,
            ans: document.getElementById("portnum").value,
            idx: random_idx[rand]
          };
          question_array.push(question_hash); //問題の番号の記録
          question_cnt++;
          random_idx.splice(rand, 1);
          r_cnt--;

          rand = Math.floor( Math.random() * r_cnt);  //次の問題の設定
          answer = data[random_idx[rand]].port;
          question = data[random_idx[rand]].protocol;

           console.log(data[random_idx[rand]].port);   //チェック用
          // console.log(data[random_idx[rand]].protocol); //チェック用

          document.getElementById("portnum").value="";  //入力ボックスの初期化
          document.getElementById("protocol").textContent=question; //吹き出し内のテキストの変換
          document.getElementById("g_alic").style.backgroundImage='url(../img/alic_sad.png)';  //画像の変更
          $('#fukidashi').animate({"top" : "0"},300); //アニメーション
          $('#fukidashi').animate({"top" : "50px"},200);  //アニメーション

        }else{
          // console.log("正解");

          question_hash = {
            port: answer,
            ans: document.getElementById("portnum").value,
            idx: random_idx[rand]
          };
          question_array.push(question_hash); //問題の番号の記録
          question_cnt++;
          random_idx.splice(rand, 1);
          r_cnt--;

          rand = Math.floor( Math.random() * r_cnt);  //次の問題の設定
          answer = data[random_idx[rand]].port;
          question = data[random_idx[rand]].protocol;

           console.log(data[random_idx[rand]].port);   //チェック用
          // console.log(data[random_idx[rand]].protocol); //チェック用

          document.getElementById("portnum").value="";  //入力ボックスの初期化
          document.getElementById("protocol").textContent=question; //吹き出し内のテキストの変換
          document.getElementById("g_alic").style.backgroundImage='url(../img/alic_grad.png)'; //画像の変更

          $('#fukidashi').animate({"top" : "0"},300); //アニメーション
          $('#fukidashi').animate({"top" : "50px"},200); //アニメーション

        }
      }else{
        question_hash = {
          port: answer,
          ans: document.getElementById("portnum").value,
          idx: random_idx[rand]
        };
        question_array.push(question_hash); //問題の番号の記録
        question_cnt++;
        go_result(question_array);  //リザルト画面へ遷移
      }
    }
  }
  //一問目をランダムで表示する処理
  $(document).ready(function(){
    document.getElementById("protocol").textContent=question;
    $('#fukidashi').animate({"top" : "0"},300); //アニメーション
    $('#fukidashi').animate({"top" : "50px"},200);  //アニメーション
  });

  });
});

//Enterキー以外のとき
function test(){
var foo = document.getElementById("portnum").value;
}

//リザルト画面へ遷移
function go_result(question_array){
  var result = question_array;
  var encodedJson = encodeURIComponent(JSON.stringify(result));
  location.href = "./Result.html#" + encodedJson;
}

function rule(){
    document.getElementById("info").style.display='block';
    $('#info').animate({ "left": "0%", "top": "-31%" }, 400);  //一回目のクリックで実行
  $('#s_button').click(function(){
    window.location.href = "../html/Game.html" //二回目のクリックで実行
    }
  );
}
