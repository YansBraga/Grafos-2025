## The House of the Seven Women (Beecrowd 3350)

[Link da solução](https://judge.beecrowd.com/en/runs/code/47543901) 

### Descrição 
Vitória!

O ano é 1840 em algum universo paralelo do multiverso, e os farrapos venceram e proclamaram a República do Brasil. Para garantir a paz e a prosperidade na nova república, os farrapos construíram rapidamente muitas estradas temporárias conectando as *2N* províncias do país. Devido à pressa com que essas estradas foram construídas, elas são muito estreitas. Assim, para cada uma delas, a Presidente Anita Garibaldi, diretamente da Casa das Sete Mulheres, determinou um único sentido em que a estrada poderia ser tomada. Curiosamente, verificou-se que de cada província partem exatamente seis estradas temporárias, embora o número de estradas que chegam a cada província seja arbitrário.

Dizemos que uma província *J* é diretamente alcançável a partir de uma província *I* se existe uma estrada temporária de *I* para *J*. Não há estradas temporárias de uma província para si mesma e, para cada par de províncias (*I, J*), existe no máximo uma estrada temporária de *I* para *J* e no máximo uma estrada temporária de *J* para *I*.

Agora, para responder às demandas do povo com mais eficiência, a Presidente quer selecionar algumas províncias para estabelecer um diretório do gabinete da presidência em cada uma delas. Qualquer província pode ser selecionada para receber ou não um diretório. Não é nem mesmo certo que a província onde fica a Casa das Sete Mulheres receberá um diretório. No entanto, cada província tem exatamente uma província gêmea e, para cada par de províncias gêmeas, apenas no máximo uma delas pode receber um diretório do gabinete da presidência, porque a Presidente não quer desperdiçar recursos públicos.

Além disso, a Presidente quer, para cada província *I* que não for selecionada para receber um diretório, que haja pelo menos duas províncias selecionadas diretamente alcançáveis a partir de *I*.

Imprima para a Presidente Anita Garibaldi uma lista viável de províncias que ela pode selecionar.

**Entrada**

A primeira linha da entrada consiste unicamente no inteiro *N* (*4 <= N <= 10^4*). Cada *I*-ésima (*1 <= I <= 2N*) das *2N* linhas seguintes consiste em exatamente 6 inteiros no intervalo *[1 .. 2N]*, representando as províncias que são diretamente alcançáveis a partir da província *I*.

Para cada *I* ímpar no intervalo *[1 .. 2N - 1]*, a província gêmea de *I* é a província *I + 1*, e vice-versa. É garantido que, para a entrada fornecida, existe uma seleção *S* de províncias que atende aos requisitos da Presidente Anita Garibaldi e, além disso, atende aos seguintes requisitos para cada província *I*:

- Se *I* não está na seleção *S*, então existem pelo menos 4 províncias selecionadas em *S* que são diretamente alcançáveis a partir de *I*;
- Se *I* está na seleção *S*, então ainda existem pelo menos 3 províncias selecionadas em *S* que são diretamente alcançáveis a partir de *I*.

**Saída**

Imprima uma lista de inteiros no intervalo *[1 .. 2N]*, separados por espaço em branco ou caracteres de fim de linha, que representam uma seleção de províncias que atendem aos requisitos da Presidente Anita Garibaldi. Não é necessário que sua seleção também atenda aos requisitos atendidos pela seleção *S* cuja existência é garantida na descrição da entrada.


| Input Samples | Output Samples |
| :--- | :--- |
| <pre>4<br>4 2 5 6 8 7<br>4 8 7 5 3 1<br>8 4 5 1 6 2<br>6 8 1 5 2 3<br>3 4 8 6 7 2<br>7 4 2 5 1 8<br>3 5 4 6 8 2<br>1 3 5 4 2 7</pre> | <pre>2<br>4<br>5<br>8</pre> |
| <pre>5<br>10 6 2 8 4 9<br>4 6 9 8 5 3<br>1 7 8 2 5 4<br>10 3 2 7 1 5<br>2 9 1 4 10 3<br>4 7 5 10 1 8<br>5 4 3 2 8 9<br>10 7 9 5 2 6<br>3 1 10 8 2 5<br>1 8 5 6 2 7</pre> | <pre>2<br>4<br>5<br>8<br>10</pre> |
| <pre>7<br>7 4 13 5 9 12<br>14 9 8 5 11 4<br>6 9 11 14 2 8<br>10 13 1 8 6 11<br>9 4 13 7 12 1<br>7 12 4 1 13 10<br>12 4 9 1 13 5<br>2 11 4 9 5 14<br>3 8 11 13 1 6<br>2 14 12 5 4 8<br>13 6 3 10 8 2<br>13 10 7 1 6 4<br>10 8 11 2 6 3<br>6 11 9 8 2 3</pre> | <pre>2<br>4<br>6<br>8<br>9<br>12<br>13</pre> |