$(function() {
  $.getJSON("well_known.json" , function(data) {

    var result = JSON.parse(decodeURIComponent(location.hash.substr(1)));
    var mis_count = 0;
    var mis_question = [];



    for(var i=0; i<5; i++){
      //document.getElementById("ano").textContent=i;
      var list_Element = document.createElement("tr");
      //このシングルクォーテーションの中に表示したいデータ（間違えた問題のデータ）を入れる
      var recode = result[i];
      if(recode["port"] != recode["ans"]){
        mis_count++;
        mis_question.push(data[recode["idx"]].protocol);
      }
      list_Element.innerHTML = '<td class="a_no" id="ano">'+ (i + 1) + '</td><td class="p_name" id="pname">' + data[recode["idx"]].protocol + '</td><td class="p_num" id="pnum">' + recode["port"] + '</td>'
      var parent_object = document.getElementById("answer_table");
      parent_object.appendChild(list_Element);

      var  td = document.getElementsByClassName("a_no")[i + 1]
      if(recode["port"] != recode["ans"]){
       td.style.backgroundColor = "#f20"

      //  mis_count++;
      //  mis_question.push(data[recode["idx"]].protocol);
     } else {
     td.style.backgroundColor = "#8f0"
    }
    }


    console.log(mis_question);
    mis_count = 5 - mis_count;
    document.getElementById("answer_num").textContent=mis_count;
    printdata = result.map(r => [data[r.idx].protocol,r.port, r.ans].join(",")).join("/");
  });
});

//プリント用関数
function r_print(){
  $.ajax("http://localhost:1234/?data=" + printdata);
  //body...
}

function restart() {
  window.location.href = "../html/Start.html"
}
